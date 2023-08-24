import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CloneRequestComponent } from './clone-request.component';

describe('CloneRequestComponent', () => {
  let component: CloneRequestComponent;
  let fixture: ComponentFixture<CloneRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CloneRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CloneRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
