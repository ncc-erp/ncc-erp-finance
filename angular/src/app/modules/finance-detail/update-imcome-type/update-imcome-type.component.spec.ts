import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateImcomeTypeComponent } from './update-imcome-type.component';

describe('UpdateImcomeTypeComponent', () => {
  let component: UpdateImcomeTypeComponent;
  let fixture: ComponentFixture<UpdateImcomeTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UpdateImcomeTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateImcomeTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
