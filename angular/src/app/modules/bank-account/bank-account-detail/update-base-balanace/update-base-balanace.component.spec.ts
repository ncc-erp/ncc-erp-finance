import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateBaseBalanaceComponent } from './update-base-balanace.component';

describe('UpdateBaseBalanaceComponent', () => {
  let component: UpdateBaseBalanaceComponent;
  let fixture: ComponentFixture<UpdateBaseBalanaceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UpdateBaseBalanaceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateBaseBalanaceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
